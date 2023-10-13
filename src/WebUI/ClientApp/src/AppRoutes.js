import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import Home from "./components/home/Home";
import SayHello from "./components/say-hello/SayHello";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/say-hello',
        requireAuth: true,
        element: <SayHello />
    },
    ...ApiAuthorzationRoutes
];

export default AppRoutes;
