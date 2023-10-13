import React, { useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './../api-authorization/LoginMenu';
import './NavMenu.css';

const NavMenu = () => {

    const [state, setState] = useState({});

    const toggleNavbar = () => {
        setState({
            collapsed: !this.state.collapsed
        });
    }

    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                <NavbarBrand tag={Link} to="/">Benivo demo app</NavbarBrand>
                <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={state.collapsed} navbar>
                    <ul className="navbar-nav flex-grow">
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/say-hello">Say hello</NavLink>
                        </NavItem>
                        <LoginMenu>
                        </LoginMenu>
                    </ul>
                </Collapse>
            </Navbar>
        </header>
    );

}

export default NavMenu;
