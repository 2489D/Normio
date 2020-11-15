import React, {useCallback, useState} from "react";
import { nanoid } from 'nanoid'

type HostLoginProps = {
    hostId?: string,
    handleSubmit: () => void,
}

const HostLogin: React.FC<HostLoginProps> = props => {
    const [loggedIn, setLoggedIn] = useState(false)
    const onSubmit = useCallback(event => {
        event.preventDefault()
        if (loggedIn) {
            return
        }
        props.handleSubmit()
        setLoggedIn(true)
    }, [])
    return (
        <div className={"card shadow-lg"}>
            <div className={"card-header font-weight-bold text-center"}>
                호스트 계정으로 로그인하세요
            </div>
            <div className={"card-text p-3"}>
                <form onSubmit={onSubmit}>
                    {!loggedIn ?
                        <React.Fragment>
                            <div className={"form-group"}>
                                <input type={"email"} className={"form-control"} placeholder={"Email"}/>
                            </div>
                            <div className={"form-group"}>
                                <input type={"password"} className={"form-control"} placeholder={"Password"}/>
                            </div>
                        </React.Fragment>
                        : <div className={"text-center m-3"}>
                            <p>Host ID</p>
                            <h2> {props.hostId} </h2>
                        </div>}
                    <input type="submit" className={`btn btn-${loggedIn ? "success" : "primary"} btn-block`}
                           value={loggedIn ? "로그인 성공" : "로그인"}/>
                </form>
            </div>
            <div className={"text-center"}>
                <p className={"font-weight-light"}> 비밀번호를 잊으셨나요? </p>
                <p className={"font-weight-light"}> 호스트 계정이 없으신가요? </p>
            </div>
        </div>
    )
}

export default HostLogin
