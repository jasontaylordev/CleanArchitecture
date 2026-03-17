import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from './api-authorization/AuthContext';
import './NavMenu.css';

function NavMenuInner() {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <>
      {isAuthenticated ? (
        <NavItem>
          <button className="btn btn-link nav-link" onClick={handleLogout}>Log out</button>
        </NavItem>
      ) : (
        <>
          <NavItem>
            <NavLink tag={Link} to="/login">Log in</NavLink>
          </NavItem>
          <NavItem>
            <NavLink tag={Link} to="/register">Register</NavLink>
          </NavItem>
        </>
      )}
    </>
  );
}

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);
    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = { collapsed: true };
  }

  toggleNavbar() {
    this.setState({ collapsed: !this.state.collapsed });
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" container>
          <NavbarBrand tag={Link} to="/">CleanArchitecture.Web</NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} to="/">Home</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/counter">Counter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/fetch-data">Fetch data</NavLink>
              </NavItem>
              <NavMenuInner />
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
