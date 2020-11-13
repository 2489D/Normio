import React, {useEffect, useState} from "react";
import {Link} from "react-router-dom";

const CurrentTime: React.FC = () => {
    const [currentTime, setCurrentTime] = useState(Date.now());

    useEffect(() => {
        setInterval(() => {
            setCurrentTime(Date.now())
        }, 1000)
    }, [])

    return (
        <div>
            <h3> CurrentTime </h3>
            <div> {currentTime} </div>
        </div>
    )
}

const GoToExam: React.FC = () => {
    return (
        <form>
            <div>
                <label>Exam Id</label>
                <input type={"text"} placeholder={"Enter Exam Id"} />
            </div>
            <div>
                <label>Exam Password</label>
                <input type={"password"} placeholder={"Enter Exam Password"}/>
            </div>
            <div>
                <Link to={"/exams/123"}>
                    <input type={"submit"} value={"Enter Exam"}/>
                </Link>
            </div>
        </form>
    )
}

const Exams = () => {
    return (
        <div>
            <h1>Exams</h1>
            <GoToExam />
            <CurrentTime />
        </div>
    )
}

export default Exams