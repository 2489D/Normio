import React from 'react';
import {BrowserRouter, Switch, Route} from "react-router-dom";

import Exams from './Exams';
import Login from './Login';
import LoginHost from './LoginHost';
import Home from './Home';
import ExamRoom from './ExamRoom';
import ExamsHost from './ExamsHost';
import ExamInspection from './ExamInspection';


const NotFound = () => (
    <h1>Not Found</h1>
)

const Root: React.FC = () => (
    <BrowserRouter>
        <Switch>
            <Route path={"/"} exact component={Home}/>
            <Route path={"/auth"} exact component={Login}/>
            <Route path={"/auth/host"} exact component={LoginHost}/>
            <Route path={"/exams"} exact component={Exams}/>
            <Route path={"/exams/host"} exact component={ExamsHost}/>
            <Route path={"/exams/123"} exact component={ExamRoom}/>
            <Route path={"/exams/123/inspection"} exact component={ExamInspection} />
            <Route component={NotFound} />
        </Switch>
    </BrowserRouter>
)

function App() {
    return (
        <div className={"container"}>
            <Root />
        </div>
    );
}

export default App;
