import React from 'react';
import { Root, NavBar } from './Routes';

import Home from './Pages/Home';
import { BrowserRouter } from 'react-router-dom';

function App() {
    return (
        <BrowserRouter>
            <NavBar />
            <div className={"container"}>
                <Root />
            </div>
        </BrowserRouter>
    );
}

export default App;
