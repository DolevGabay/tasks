import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Home.css';

const Home = () => {
  const [user, setUser] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleUserChange = (e) => {
    setUser(e.target.value);
  };

  const handlePasswordChange = (e) => {
    setPassword(e.target.value);
  };

  const handleSignup = async (e) => {
    e.preventDefault();

    try {
        const response = await fetch('http://localhost:8080/signup', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ user, password }),
        });

        if (response.ok) {
            const result = await response.json();
            console.log('Signup successful:', result);
            if (result == true){
                alert('Signup successful');
            }
            else{
                alert('Signup failed');
            }
        } else {
            console.error('Signup failed');
            alert('Signup failed user already exist');
        }
    } catch (error) {
        console.error('Error during signup:', error);
    }
};

const handleLogin = async (e) => {
  e.preventDefault();
  
  const url = 'http://localhost:8080/login?user=' + encodeURIComponent(user) + '&password=' + encodeURIComponent(password);

  try {
      const response = await fetch(url, {
          method: 'GET',
          headers: {
              'Content-Type': 'application/json',
          },
      });

      if (response.ok) {
          const result = await response.json();
          console.log('Login is:', result);
          if (result == true){
            navigate('/tasks', { state: { username: user  } });
          }
      } else {
          console.error('Login failed try again');
          alert('Login failed try again');
      }
  } catch (error) {
      console.error('Error during login:', error);
  }
};

  return (
    <div className="main">
      <input type="checkbox" id="chk" aria-hidden="true" />

      <div className="signup">
        <form onSubmit={handleSignup}>
          <label htmlFor="chk" aria-hidden="true">
            Sign up
          </label>
          <input
            type="text"
            name="txt"
            placeholder="User name"
            value={user}
            onChange={handleUserChange}
            required
          />
          <input
            type="password"
            name="pswd"
            placeholder="Password"
            value={password}
            onChange={handlePasswordChange}
            required
          />
          <button type="submit">Sign up</button>
        </form>
      </div>

      <div className="login">
        <form onSubmit={handleLogin}>
          <label htmlFor="chk" aria-hidden="true">
            Login
          </label>
          <input
            type="text"
            name="txt"
            placeholder="User name"
            value={user}
            onChange={handleUserChange}
            required
          />
          <input
            type="password"
            name="pswd"
            placeholder="Password"
            value={password}
            onChange={handlePasswordChange}
            required
          />
          <button type="submit">Login</button>
        </form>
      </div>
    </div>
  );
};

export default Home;
