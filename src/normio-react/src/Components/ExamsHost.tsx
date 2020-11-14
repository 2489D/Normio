import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import CurrentTime from "./CurrentTime";

const CreateExam: React.FC = () => {
    return (
        <form>
            <div>
                <label>Exam Id</label>
                <input type={"text"} placeholder={"Enter Exam Id"} />
            </div>
            <div>
                <label>Exam Password</label>
                <input type={"password"} placeholder={"Enter Exam Password"} />
            </div>
            <div>
                <Link to={"/exams/123"}>
                    <input type={"submit"} value={"Create Exam"} />
                </Link>
            </div>
        </form>
    )
}

const ExamsHost = () => {
    return (
        <div>
            <h1>Exams for Host</h1>
            <CurrentTime />
            <div className="container">
                <div className="row">
                    <div className="col">
                        <h2>Enter Exam</h2>
                    </div>
                    <div>
                        <h2>Create Exam</h2>
                        <CreateExam />
                    </div>
                </div>
            </div>
        </div>
    )
}

export default ExamsHost