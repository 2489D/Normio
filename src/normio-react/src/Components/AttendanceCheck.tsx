import { nanoid } from 'nanoid'
import React, { useRef, useState } from 'react'

// FIXME: easy to be attacked by a bot
const AttendanceCheck: React.FC = () => {
    const checkString = useRef(nanoid(14))
    return (
        <form>
            <div className="card shadow-lg m-3">
                <div className="card-header">출석 체크</div>
                <div className="card-body">
                    <div className="form-group">
                        <label>다음을 정확하게 입력하세요: {checkString.current}</label>
                        <input className="form-control" type="text" />
                    </div>
                    <div className="form-group">
                        <input className="form-control btn btn-primary" type="submit" value="출석 체크" />
                    </div>
                    <p className="font-weight-light">
                        부정행위안함!
                    </p>
                </div>
            </div>
        </form>
    )
}

export default AttendanceCheck