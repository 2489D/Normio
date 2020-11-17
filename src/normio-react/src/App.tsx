import React, {useState} from 'react';
import { Root, NavBar } from './Routes';

import Home from './Pages/Home';
import { BrowserRouter } from 'react-router-dom';
import {ExamContext, ExamReadModel} from './Context/ExamContext';

function App() {
    const [exam, setExam] = useState<ExamReadModel | null>(null)
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
