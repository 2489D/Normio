import React, {useCallback, useContext, useEffect, useState} from "react";
import AttendanceCheck from "../Components/AttendanceCheck";
import {ExamContext} from "../Context/ExamContext";
import {Link} from "react-router-dom";

const ExamRoom: React.FC = () => {
    const { exam, updateExam } = useContext(ExamContext);
    const [checked, setChecked] = useState(false);
    const [leftTime, setLeftTime] = useState(0);
    
    const formatTime = useCallback((seconds: number) => {
        const hours = Math.trunc(seconds / 3600)
        const minutes = Math.trunc((seconds - hours * 3600) / 60)
        const secs = Math.trunc(seconds - hours * 3600 - minutes * 60)
        return `${hours}시간 ${minutes}분 ${secs}초`
    }, [])
    
    useEffect(() => {
        if (exam) {
            const startTime =  Date.parse(exam.startDateTime) / 1000
            const endTime = startTime + exam.durationMins * 60
            const leftTime = Math.trunc(endTime - Date.now() / 1000)
            setLeftTime(leftTime)
            setInterval(() => {
                const leftTime = Math.trunc(endTime - Date.now() / 1000)
                setLeftTime(leftTime)
            }, 1000)
        }
    }, [])
    
    const handleCheck = useCallback(() => {
        setChecked(true)
    }, []);
    
    if (exam === null) {
        return (
            <div className={"container"}>
                <Link to={"/"}>
                    시험 로드 실패! 다시 입장해주세요.
                </Link>
            </div>
        )
    }

    return (
        <div className={"container"}>
            {!checked &&
            <div className={"row my-5"}>
                <div className={"col"}>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <h1 className={"mx-auto my-5"}>환영합니다!</h1>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <h2 className={"mx-auto my-2"}>CS320 Final</h2>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <h2 className={"mx-auto my-2"}>
                                { exam?.status.BeforeExam ?
                                    "시험이 아직 시작하지 않았습니다." : null
                                }
                            </h2>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <p className={"font-weight-light mx-auto my-1"}>시작까지 37분 28초 남았습니다.</p>
                        </div>
                    </div>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <p className={"font-weight-light mx-auto my-1"}>27분 38초 후에 입장이 종료됩니다.</p>
                        </div>
                    </div>
                </div>
                <div className={"col"}>
                    <div className={"row"}>
                        <div className={"col d-flex"}>
                            <div className={"mx-auto"}>
                                <AttendanceCheck handleCheck={handleCheck}/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            }
            {checked &&
            <React.Fragment>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <h1 className={"mx-auto mt-3 mb-0"}>
                            { exam?.status.BeforeExam ?
                                "시험이 아직 시작하지 않았습니다." : null
                            }
                        </h1>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <p className={"font-weight-light mx-auto my-3"}>{formatTime(leftTime)} 뒤에 종료됩니다</p>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col"}>
                        <div className={"container"}>
                            <div className={"row"}>
                                <div className={"col"}>
                                    <div className={"card"}>
                                        <div className={"card-body"}>
                                            <p className={"card-text"}>
                                                시험 감독 홍길동
                                            </p>
                                        </div>
                                    </div>
                                </div>
                                <div className={"col"}>
                                    <div className={"card"}>
                                        <div className={"card-body"}>
                                            시험 감독 홍길동
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col"}>
                        <h2>시험 문제</h2>
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
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header font-weight-bold"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text font-weight-light"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={"col-xs-6 col-md-4 col-lg-3"}>
                        <div className={"card shadow-lg m-3"}>
                            <div className={"card-header"}>문제 2번 예외 조건...</div>
                            <div className={"card-body"}>
                                <div className={"card-text"}>
                                    2번 문항의 A 조건에 대해서
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>
            }
        </div>
    );
}

export default ExamRoom;