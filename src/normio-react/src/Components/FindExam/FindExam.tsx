import React, { useCallback, useState } from 'react'
import ExamInfo from '../ExamInfo/ExamInfo';

const FindExam: React.FC = props => {
    const [exam, setExam] = useState<string | null>(null);
    const handleSubmit = useCallback((e) => {
        e.preventDefault();
        setExam("CS330");
    }, []);
    return (
        <React.Fragment>
            <div className="container">
                <div className="row">
                    <div className="col"></div>
                    <div className="col">
                        <form className="card shadow-lg p-3" onSubmit={handleSubmit}>
                            <div className="form-group">
                                <label>시험 ID를 입력하세요</label>
                                <input className="form-control" type={"text"} placeholder={"Exam Id"} />
                            </div>
                            <div className="form-group">
                                <label>암호를 입력하세요</label>
                                <input className="form-control" type={"password"} placeholder={"Exam Password"} />
                            </div>
                            <div className="form-group">
                                <input className="from-control btn btn-primary btn-block" type={"submit"} value={"시험을 찾습니다"} />
                            </div>
                        </form>
                    </div>
                    <div className="col"></div>
                </div>
                <div className={`row my-3 ${exam ? "" : "d-none"}`}>
                    <div className="col"></div>
                    <div className="col">
                        <ExamInfo id="123" title={exam ?? ""} host="ㄹㅇ" start={"내일"}></ExamInfo>
                    </div>
                    <div className="col"></div>
                </div>
            </div>
        </React.Fragment>
    )
}

export default FindExam