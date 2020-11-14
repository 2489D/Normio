import React from "react";

const ExamInspection: React.FC = () => {
    const logs = [
        "ExamOpened 2020-11-03 08:30:31"
    ]
    return (
        <div>
            <h1>Exam Inspection</h1>
            <div>
                <h2>Exam Info</h2>
                <div> Exam Id : 1231231231</div>
                <div> # Students : 47</div>
            </div>
            <div>
                <h2>Logs</h2>
                <div>
                    {logs.map((log, index) => (
                        <div key={`${log}-${index}`}>{log}</div>
                    ))}
                </div>
            </div>
        </div>
    )
}

export default ExamInspection;