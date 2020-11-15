import React from 'react'
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import Home from '../Pages/Home';
import ExamInspection from '../Pages/ExamInspection';
import ExamRoom from '../Pages/ExamRoom';
import FindExam from '../Pages/FindExam';
import CreateExamPage from "../Pages/CreateExam";

const NotFound = () => (
    <h1>Not Found</h1>
)

const Root: React.FC = () => (
    <Switch>
        <Route path={"/"} exact component={Home} />
        <Route path={"/exams"} exact component={FindExam} />
        <Route path={"/exams/create"} exact component={CreateExamPage} />
        <Route path={"/exams/123"} exact component={ExamRoom} />
        <Route path={"/exams/123/inspection"} exact component={ExamInspection} />
        <Route component={NotFound} />
    </Switch>
)

export default Root