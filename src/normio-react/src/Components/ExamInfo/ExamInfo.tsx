import React, { useState } from 'react';
import {Link} from "react-router-dom";

type ExamInfoProps = {
    id: string,
    title: string,
    host: string,
    start: string,
    description?: string,
}

const ExamInfo: React.FC<ExamInfoProps> = props => {
    const [mouse, setMouse] = useState(false);
    return (
        <div className="card shadow-lg">
            <div className="font-weight-bold card-header">
                시험을 찾았습니다!
            </div>
            <div className="card-body">
                <h5 className="card-title">
                    {props.title}
                </h5>
                <p className="card-text">
                    시작: {props.start}
                </p>
                {props.description ?
                    <p className="card-text">
                        {props.description}
                    </p> : null}
                <Link to={"/exams/123"}>
                    <button className={`btn btn-block btn-${mouse ? "primary" : "success"} font-weight-light`}
                            onMouseOver={() => setMouse(true)}
                            onMouseLeave={() => setMouse(false)}
                    >
                        {mouse ? "네" : "찾던 시험이 맞나요?"}
                    </button>
                </Link>
            </div>
        </div>
    )
}

export default ExamInfo