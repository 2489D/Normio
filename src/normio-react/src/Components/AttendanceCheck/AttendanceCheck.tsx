import { nanoid } from 'nanoid'
import React, {useCallback, useContext, useRef, useState} from 'react'
import {ExamContext} from "../../Context/ExamContext";
import {useForm} from "react-hook-form";
import NormioApi from "../../API";

// FIXME: easy to be attacked by a bot

type AttendanceCheckProps = {
    handleCheck: () => void,
}

const AttendanceCheck: React.FC<AttendanceCheckProps> = props => {
    const { exam } = useContext(ExamContext)
    const { register, handleSubmit } = useForm()
    const [error, setError] = useState<string | null>(null)
    
    const onSubmit = useCallback(async data => {
        if (exam === null) {
            setError("시험 데이터를 불러 오지 못했습니다.")
            return
        }
        const { studentId, name } = data
        try {
            await NormioApi.letStudentIn(exam.id, studentId, name)
            props.handleCheck();
        } catch (err) {
            if (err.response?.data) {
                setError(err.response.data)
            }
            setError("알 수 없는 에러가 발생했습니다.")
        }
    }, [props.handleCheck])
    return (
        <div className={"container"}>
            <div className={"row"}>
                <div className={"col d-flex"}>
                    <form className={"mx-auto"} onSubmit={handleSubmit(onSubmit)}>
                        <div className="card shadow-lg m-3">
                            <div className="card-header font-weight-bold">출석 체크</div>
                            <div className="card-body">
                                <div className="form-group">
                                    <label>부여받은 학생 ID를 입력하세요.</label>
                                    <input className="form-control" name={"studentId"} ref={register} type="text" placeholder={"학생 ID"} />
                                </div>
                                <div className="form-group">
                                    <label>입장 후 표시할 이름을 입력하세요</label>
                                    <input className="form-control" name={"name"} ref={register} type="text" placeholder={"이름"} />
                                </div>
                                <div className="form-group">
                                    <input className={"form-control btn " + "btn-primary"} type="submit" value="시험에 입장합니다" />
                                </div>
                                <p className="font-weight-light">
                                    출석 체크를 함으로써 시험을 보는 도중 부정 행위를 하지 않을 것임을 서약합니다.
                                </p>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}

export default AttendanceCheck