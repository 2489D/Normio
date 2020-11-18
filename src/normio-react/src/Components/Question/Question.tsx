import React from "react";
import {Normio} from "../../Context/ExamContext";

const Question: React.FC<{ question: Normio.Question}> = ({ question, ...props }) => {
    return (
        <tr>
            <td scope={"row"}>{question.title}</td>
            <td>{question.description}</td>
            <td>{question.timestamp}</td>
            <td>
                <button className={"btn btn-sm btn-primary"}>
                    받기
                </button>
            </td>
        </tr>
    )
}

const Questions: React.FC<{questions: Normio.Question[]}> = ({ questions, ...props }) => {
    return (
        <table className={"table table-sm"}>
            <thead>
                <tr>
                    <th scope={"col"}>문제 이름</th>
                    <th scope={"col"}>문제 설명</th>
                    <th scope={"col"}>등록</th>
                    <th scope={"col"}>다운로드</th>
                </tr>
            </thead>
            <tbody>
                {questions.map(question => (
                    <Question question={question} />
                ))}
            </tbody>
        </table>
    )
}

export default Questions
