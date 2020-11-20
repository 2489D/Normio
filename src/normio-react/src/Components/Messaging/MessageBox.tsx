import React, {useCallback, useContext} from "react";
import {ExamContext} from "../../Context/ExamContext";
import {useForm} from "react-hook-form";
import NormioApi, {Normio} from "../../API";

type StudentMessageBoxProps = {
    studentId: string,
    receiverIds: string[],
}

const StudentMessageBox: React.FC<StudentMessageBoxProps> = props => {
    const { exam } = useContext(ExamContext)
    const { register, handleSubmit } = useForm()
    const examId = exam?.id

    const onSubmit = useCallback(async data => {
        const { content } = data;
        if (examId) {
            await NormioApi.sendMessage(
                examId, Normio.MessageKind.FromStudentToHost, props.studentId, props.receiverIds, content
            )
        }
    }, [examId])

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className={"form-group"}>
                <input className={"form-control"} name={"content"} ref={register} />
            </div>
        </form>
    )
}

export default StudentMessageBox