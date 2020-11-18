import React from "react";
import {Normio} from "../../Context/ExamContext";
import {formatTime} from "../../Utils";

const ExamGreeting: React.FC<{exam: Normio.Exam}> = ({ exam, ...props}) => {
    
    const examStatus = () => {
        const status = Object.keys(exam.status)[0]
        if (status === "BeforeExam") {
            return "시험이 아직 시작하지 않았습니다"
        }
        if (status === "During") {
            return "시험이 진행중 입니다"
        }
        if (status === "BeforeExam") {
            return "시험이 이미 종료 되었습니다"
        }
    }
    
    const timeLeft = formatTime(Date.parse(exam.startDateTime) / 1000 - Date.now() / 1000)
    
    return (
        <div className={"container"}>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <h1 className={"mx-auto"}>환영합니다!</h1>
                </div>
            </div>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <h2 className={"mx-auto"}>{exam.title}</h2>
                </div>
            </div>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <h2 className={"mx-auto"}>{examStatus()}</h2>
                </div>
            </div>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <p className={"font-weight-light mx-auto my-1"}>시작까지 {timeLeft} 남았습니다.</p>
                </div>
            </div>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <p className={"font-weight-light mx-auto my-1"}>시험은 {exam.durationMins} 분간 진행됩니다.</p>
                </div>
            </div>
        </div>
    )
}

export default ExamGreeting