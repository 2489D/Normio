import React, { useState } from 'react';

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
            <div className="card-header">
                시험 정보
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
                <div className="card-text my-1 d-flex">
                    <div className="font-weight-light mx-auto">
                        찾던 시험이 맞나요?
                    </div>
                </div>
                <button className={`btn btn-block btn-${mouse ? "primary" : "success"}`}
                    onMouseOver={() => setMouse(true)}
                    onMouseLeave={() => setMouse(false)}
                >
                    네
                </button>
            </div>
        </div>
    )
}

export default ExamInfo