import React from 'react'
import { Link } from 'react-router-dom';
import CurrentTime from '../Components/CurrentTime';


const NavBar: React.FC = props => {
    return (
        <nav className="navbar navbar-light bg-light">
            <Link to="/">
                <div className="navbar-brand font-weight-bold">
                    Normio
                </div>
            </Link>
            <CurrentTime />
        </nav>
    )
}

export default NavBar