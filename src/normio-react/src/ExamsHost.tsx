import React, {useEffect, useState} from "react";
import {Link} from "react-router-dom";

import { Container, Row, Col } from 'reactstrap';

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
                    <input type={"submit"} value={"Create Exam"}/>
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
            <br /><br />

            <div>
                <h2>

                </h2>
            </div>


            <Container>
                <Row>
                    <Col>
                        <h2>Enter Exam</h2>
                        <GoToExam />
                    </Col>
                    <Col>
                        <h2>Create Exam</h2>
                        <CreateExam />
                    </Col>
                </Row>
            </Container>

            
        </div>
    )
}

export default ExamsHost