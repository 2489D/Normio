import React from 'react'
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import ExamInspection from '../Pages/ExamInspection';
import ExamRoom from '../Pages/ExamRoom';
import Exams from '../Pages/Exams';
import Home from '../Pages/Home';

const NotFound = () => (
    <h1>Not Found</h1>
)

const Root: React.FC = () => (
    <Switch>
        <Route path={"/"} exact component={Home} />
        <Route path={"/exams"} exact component={Exams} />
        <Route path={"/exams/123"} exact component={ExamRoom} />
        <Route path={"/exams/123/inspection"} exact component={ExamInspection} />
        <Route component={NotFound} />
    </Switch>
)

export default Root