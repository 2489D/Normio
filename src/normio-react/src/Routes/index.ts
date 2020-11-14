import Root from './Root';
import NavBar from './Navbar';

export const routes = [
    {
        name: "Home",
        route: "/",
    },
    {
        name: "Account",
        route: "/auth/account",
    },
    {
        name: "Login",
        route: "/auth/login",
    }
]

export {
    Root,
    NavBar,
}
