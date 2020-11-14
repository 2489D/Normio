import { nanoid } from 'nanoid'
import React, {useCallback, useRef, useState} from 'react'

// FIXME: easy to be attacked by a bot

type AttendanceCheckProps = {
    handleCheck: () => void,
}

const AttendanceCheck: React.FC<AttendanceCheckProps> = props => {
    const checkString = useRef(nanoid(14))
    const onSubmit = useCallback((e) => {
        e.preventDefault();
        props.handleCheck();
    }, [props.handleCheck])
    return (
        <form onSubmit={onSubmit}>
            <div className="card shadow-lg m-3">
                <div className="card-header font-weight-bold">출석 체크</div>
                <div className="card-body">
                    <div className="form-group">
                        <label>다음을 정확하게 입력하세요: {checkString.current}</label>
                        <input className="form-control" type="text" placeholder={"I am not a robot"} />
                    </div>
                    <div className="form-group">
                        <label>부여받은 학생 ID를 입력하세요.</label>
                        <input className="form-control" type="text" placeholder={"Student Id"} />
                    </div>
                    <div className="form-group">
                        <input className="form-control btn btn-primary" type="submit" value="시험에 입장합니다" />
                    </div>
                    <p className="font-weight-light">
                        출석 체크를 함으로써 시험을 보는 도중 부정 행위를 하지 않을 것임을 서약합니다.
                    </p>
                </div>
            </div>
        </form>
    )
}

export default AttendanceCheck