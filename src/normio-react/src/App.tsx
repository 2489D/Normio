import React from 'react';
import {BrowserRouter, Switch, Route} from "react-router-dom";

import NavBar from './Components/Navbar';
import Login from './Login';
import Home from './Home';
import Exams from './Components/Exams';
import ExamRoom from './Components/ExamRoom';
import ExamInspection from './Components/ExamInspection';

const NotFound = () => (
    <h1>Not Found</h1>
)

const Root: React.FC = () => (
    <BrowserRouter>
        <Switch>
            <Route path={"/"} exact component={Home}/>
            <Route path={"/auth"} exact component={Login}/>
            <Route path={"/exams"} exact component={Exams}/>
            <Route path={"/exams/123"} exact component={ExamRoom}/>
            <Route path={"/exams/123/inspection"} exact component={ExamInspection} />
            <Route component={NotFound} />
        </Switch>
    </BrowserRouter>
)

function App() {
    return (
        <div className={"container"}>
            <NavBar />
            <Root />
        </div>
    );
}

export default App;
