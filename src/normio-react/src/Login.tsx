import React from "react";
import {Link} from 'react-router-dom'

const Login = () => {
    return (
        <div className={"container"}>
            <div className={"row"}>
                <div className={"col"}>
                    <h1>Login</h1>
                </div>
            </div>
            <div className={"row"}>
                <div className={"col"}>
                    <form>
                        <div className={"m-1"}>
                            <input type="email" placeholder={"Email"} />
                        </div>
                        <div className={"m-1"}>
                            <input type="password" placeholder={"Password"} />
                        </div>
                        <div className={"m-1"}>
                            <Link to={"/exams"}>
                                <input type={"submit"} value={"Login"} />
                            </Link>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}

export default Login
