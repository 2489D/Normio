import React, { useState } from 'react';
import {Link} from "react-router-dom";

type ExamInfoProps = {
    exam : any
}

const ExamInfo: React.FC<ExamInfoProps> = ({ exam, ...props }) => {
    const [mouse, setMouse] = useState(false);
    return (
        <div className="card shadow-lg">
            <div className="font-weight-bold card-header">
                시험을 찾았습니다!
            </div>
            <div className="card-body">
                <h5 className="card-title">
                    {exam.title}
                </h5>
                <p className="card-text">
                    {exam.startDateTime ? exam.startDateTime + "에 시작합니다." : "아직 시작 시간이 정해지지 않았습니다"}
                </p>
                <Link to={"/exams/123"}>
                    <button className={`btn btn-block btn-${mouse ? "primary" : "success"} font-weight-light`}
                            onMouseOver={() => setMouse(true)}
                            onMouseLeave={() => setMouse(false)} >
                        {mouse ? "네" : "찾던 시험이 맞나요?"}
                    </button>
                </Link>
            </div>
        </div>
    )
}

export default ExamInfo