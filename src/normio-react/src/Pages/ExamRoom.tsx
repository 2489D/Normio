import React, {useCallback, useContext, useEffect, useState} from "react";
import AttendanceCheck from "../Components/AttendanceCheck";
import {ExamContext} from "../Context/ExamContext";
import {Link} from "react-router-dom";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {config} from "../config/development";
import NormioApi from "../API";
import Questions from "../Components/Question";
import HostLive from "../Components/HostLive";
import ExamGreeting from "../Components/ExamGreeting";
import {formatTime} from "../Utils";
import SelfVideo from "../Components/WebRTC/SelfVIdeo";

const ExamRoom: React.FC = () => {
    const { exam, updateExam } = useContext(ExamContext);
    const [checked, setChecked] = useState(false);
    const [leftTime, setLeftTime] = useState(0);
    const [eventsReceived, setEventsReceived] = useState<any[]>([]);
    
    const handleReceiveEvent = useCallback(async () => {
        if (exam === null) {
            return
        }
        const response = await NormioApi.getExam(exam.id)
        updateExam(response.data)
    }, [])
    
    useEffect(() => {
        let ticker: NodeJS.Timeout;
        const conn = new HubConnectionBuilder()
            .withUrl(config.backendUrl + '/signal')
            .withAutomaticReconnect()
            .build()
        
        conn.start().then(result => {
            console.debug("connection established!")
            conn.on('ReceiveEvent', event => {
                console.debug(event)
                handleReceiveEvent()
            })
        }).catch(err => {
            console.debug("connection failed: ", err)
        })
        if (exam) {
            const startTime =  Date.parse(exam.startDateTime) / 1000
            const endTime = startTime + exam.durationMins * 60
            const leftTime = Math.trunc(endTime - Date.now() / 1000)
            setLeftTime(leftTime)
            ticker = setInterval(() => {
                const leftTime = Math.trunc(endTime - Date.now() / 1000)
                setLeftTime(leftTime)
            }, 1000)
        }
        
        return () => {
            conn.stop().then(res => {
                console.debug("Connection gracefully stopped")
            }).catch(err => {
                console.debug("Connection disgracefully stopped")
            })
            clearInterval(ticker)
        }
    }, [])
    
    const handleCheck = useCallback(() => {
        setChecked(true)
    }, []);
    
    if (exam === null) {
        return (
            <div className={"container"}>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <h2 className={"mx-auto m-5"}>
                            <Link to={"/"}>
                                시험 로드 실패! 다시 입장해주세요.
                            </Link>
                        </h2>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className={"container"}>
            {!checked &&
            <div className={"row my-5"}>
                <ExamGreeting exam={exam} />
                <AttendanceCheck handleCheck={handleCheck}/>
            </div>
            }
            {checked &&
            <React.Fragment>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <h1 className={"mx-auto mt-3 mb-0"}>
                            시험이 아직 시작하지 않았습니다.
                        </h1>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <p className={"font-weight-light mx-auto my-3"}>{formatTime(leftTime)} 뒤에 종료됩니다</p>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <p className={"font-weight-light mx-auto my-3"}>
                            <SelfVideo />
                        </p>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col"}>
                        <h2>시험감독</h2>
                        <div className={"row"}>
                            { exam.hosts.length > 0 ? exam.hosts.map(host => {
                                return (
                                    <div className={"col-6 my-1"}>
                                        <HostLive host={host} />
                                    </div>
                                )
                            }) : <p>아직 호스트가 없어요!</p>}
                        </div>
                    </div>
                    <div className={"col"}>
                        <h2>시험 문제</h2>
                        <Questions questions={exam.questions} />
                        <h2>답안 제출</h2>
                        <div>
                            <form className={"form-group"} onSubmit={e => alert(JSON.stringify(e))}>
                                <div className="input-group">
                                    <div className="custom-file">
                                        <input type="file" className="custom-file-input" id="inputGroupFile04" aria-describedby="inputGroupFileAddon04" />
                                        <label className="custom-file-label" htmlFor="inputGroupFile04">답안 선택</label>
                                    </div>
                                    <div className="input-group-append">
                                        <button className="btn btn-outline-secondary" type="button" id="inputGroupFileAddon04">제출</button>
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
                <div className={"row"}>
                </div>
            </React.Fragment>
            }
        </div>
    );
}

export default ExamRoom;