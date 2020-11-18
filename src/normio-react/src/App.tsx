import React, {useState} from 'react';
import { Root, NavBar } from './Routes';

import Home from './Pages/Home';
import { BrowserRouter } from 'react-router-dom';
import {ExamContext, Normio} from './Context/ExamContext';

function App() {
    const [exam, setExam] = useState<Normio.Exam | null>(null)
    return (
        <BrowserRouter>
            <ExamContext.Provider value={{
                exam: exam,
                updateExam: (data) => setExam(data),
            }}>
                <NavBar />
                <div className={"container"}>
                    <Root />
                </div>
            </ExamContext.Provider>
        </BrowserRouter>
    );
}

export default App;
