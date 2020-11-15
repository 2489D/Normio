import React, {useCallback, useEffect, useState} from "react";
import axios from 'axios';
import AttendanceCheck from "../Components/AttendanceCheck";

const baseUrl = "http://localhost:6546/"

const ExamRoom: React.FC = props => {
    const [checked, setChecked] = useState(false);

    const handleCheck = useCallback(() => {
        setChecked(true)
    }, []);

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
                            <h2 className={"mx-auto my-2"}>시험이 아직 시작하지 않았습니다.</h2>
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
                        <h1 className={"mx-auto mt-3 mb-0"}>시험이 진행중입니다</h1>
                    </div>
                </div>
                <div className={"row"}>
                    <div className={"col d-flex"}>
                        <p className={"font-weight-light mx-auto my-3"}>1시간 12분 36초 뒤에 종료됩니다</p>
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