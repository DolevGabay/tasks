import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './Tasks.css';
import { removeTask, checkInput, checkUser, sortTasks, getOpenTasks, getThisWeekTasks, getAllTasks, getClosedTasks, getStatisticsByPeriod, deleteUser } from './TasksUtills';
import { API_BASE_URL } from './config';

const Tasks = () => {
  const location = useLocation();
  const { username } = location.state || {}; 
  const navigate = useNavigate();

  const [tasks, setTasks] = useState([]);
  const [showStatistics, setShowStatistics] = useState(false);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [users, setUsers] = useState([]);
  const [isEditing, setIsEditing] = useState(false);
  const [editTaskId, setEditTaskId] = useState('');
  const [formValues, setFormValues] = useState({
    subject: '',
    description: '',
    urgency: '',
    status: '',
    date: '',
    endingDate: '',
    userInCharge: '',
  });
  const [editFormValues, setEditFormValues] = useState({
    subject: '',
    userInCharge: '',
    urgency: 1,
    status: '',
    description: '',
    date: '',
    endingDate: '',
  });
  

  // Function to add a new task
  const addTask = async () => {
    if(isEditing){
      editTask();
      return;
    }
  
    try {
      if (!checkInput(formValues)) {
        console.log('Input not valid');
        return;
      }

      const userExists = await checkUser(formValues.userInCharge);
      console.log(userExists);
  
      if (userExists === false) {
        alert('User in charge does not exist. Please enter a valid user.');
        return;
      }
  
      const response = await fetch(`${API_BASE_URL}/add-task`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formValues),
      });
  
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
  
      const data = await response.json();
      console.log('Task added successfully:', data);
  
      setTasks(data);
  
      // Clear the form after adding a task
      setFormValues({
        subject: '',
        description: '',
        urgency: '',
        status: '',
        date: '',
        endingDate: '',
        userInCharge: '',
      });
    } catch (error) {
      console.error('Error adding task:', error);
    }
  };

  const editTask = async () => {
    console.log('Editing task');
    try {
      if (!checkInput(editFormValues)) {
        console.log('Input not valid for editing');
        return;
      }

      const userExists = await checkUser(editFormValues.userInCharge);
  
      if (userExists === false) {
        alert('User in charge does not exist. Please enter a valid user.');
        return;
      }
      editFormValues.uuid = editTaskId;
  
      const response = await fetch(`${API_BASE_URL}/edit-task`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(editFormValues),
      });
  
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
  
      const data = await response.json();
      console.log('Task edited successfully:', data);
  
      setTasks(data);
      setIsEditing(false);
  
      // Clear the form after adding a task
      setFormValues({
        subject: '',
        description: '',
        urgency: '',
        status: '',
        date: '',
        endingDate: '',
        userInCharge: '',
      });
    } catch (error) {
      console.error('Error editing task:', error);
    }
  };
  
  const handleRemoveTask = async (taskId) => {
    removeTask(taskId, setTasks, setFormValues);
  };

  const handleSortTasks = async () => {
    sortTasks(setTasks);
  };

  const handleOpenTasks = async () => {
    getOpenTasks(setTasks);
  };

  const handleGetThisWeekTasks = async () => {
    getThisWeekTasks(setTasks);
  };

  const handleGetAllTasks = async () => {
    getAllTasks(setTasks);
  };

  const handleGetClosedTasks = async () => {
    getClosedTasks(setTasks);
  };

  const handleStatisticsByPeriod = async () => {
    getStatisticsByPeriod(setUsers, startDate, endDate);
  };

  const handleDeleteUser = async () => {
    if(deleteUser(username)){
      navigate('/');
    }
  };

  // Function to handle form input changes
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    if(isEditing){
      setEditFormValues({ ...editFormValues, [name]: value });
    }
    else{
      setFormValues({ ...formValues, [name]: value });
    }
  };
  
  const handleEditClick = (task) => {
    const startDate = new Date(task.Date);
    startDate.setDate(startDate.getDate() + 1); 
  
    const endDate = task.EndingDate ? new Date(task.EndingDate) : null;
    if (endDate) {
      endDate.setDate(endDate.getDate() + 1); 
    }
  
    setEditFormValues({
      subject: task.Subject,
      userInCharge: task.UserInCharge,
      urgency: task.Urgency,
      status: task.Status,
      description: task.Description,
      date: startDate.toISOString().split('T')[0], // Format date
      endingDate: endDate ? endDate.toISOString().split('T')[0] : '', 
    });
    setIsEditing(true);
    setEditTaskId(task.JobUuid);
  };
  
  useEffect(() => {
    getAllTasks(setTasks);
  }, []); 
  
  return (
    <div className="tasks-container">
      <h2 className="tasks-heading">
        Hello {username}
        <button className="delete-button" onClick={handleDeleteUser}>Delete User</button>
      </h2>
      <h2 className="tasks-heading">Welcome To Tasks Manager </h2>
      <ul className="tasks-list">
        {tasks.map((task) => (
          <li key={task.JobUuid} className="task-item">
            <div className="task-details">
              <strong>Subject:</strong> {task.Subject}
              <br />
              <strong>Description:</strong> {task.Description}
              <br />
              <strong>Urgency:</strong> {task.Urgency}
              <br />
              <strong>Status:</strong> {task.Status}
              <br />
              <strong>Date:</strong> {new Date(task.Date).toLocaleDateString()}
              <br />
              <strong>Ending Date:</strong>{' '}
              {task.EndingDate ? new Date(task.EndingDate).toLocaleDateString() : 'N/A'}
              <br />
              <strong>User In Charge:</strong> {task.UserInCharge}
            </div>
            <div className="button-container">
              <button className="remove-button" onClick={() => handleRemoveTask(task.JobUuid)}>
                Remove
              </button>
              <button className="edit-button" onClick={() => handleEditClick(task)}>
                Edit
              </button>
            </div>
          </li>
        ))}
      </ul>
      <div className="tasks-input-container">
      <form className="task-form">
      <div className="form-row">
      <input
          type="text"
          name="subject"
          placeholder="Enter subject"
          value={isEditing ? editFormValues.subject : formValues.subject}
          onChange={handleInputChange}
          required
        />
        <input
          type="text"
          name="userInCharge"
          placeholder="Enter user in charge"
          value={isEditing ? editFormValues.userInCharge :formValues.userInCharge}
          onChange={handleInputChange}
          required
        />
        
        <input
          type="number"
          name="urgency"
          placeholder="Enter urgency (1-5)"
          value={isEditing ? editFormValues.urgency :formValues.urgency}
          onChange={handleInputChange}
          min="1"
          max="5"
          required
        />
      </div>
      <div className="form-row">
      <select
            name="status"
            value={isEditing ? editFormValues.status :formValues.status}
            onChange={handleInputChange}
            required
          >
            <option value="">Select status</option>
            <option value="pending">Pending</option>
            <option value="canceled">Canceled</option>
            <option value="in progress">In Progress</option>
            <option value="done">Done</option>
          </select>

        
        <input
          type="text"
          name="description"
          placeholder="Enter description"
          value={isEditing ? editFormValues.description :formValues.description}
          onChange={handleInputChange}
        />
      </div>
      <div className="form-row">
        <div className="date-input-container">
          <label htmlFor="date" className="date-label">
            Select start date:
          </label>
          <input
            type="date"
            id="date"
            name="date"
            value={isEditing ? editFormValues.date :formValues.date}
            onChange={handleInputChange}
            className="date-input"
            required
          />
        </div>

        <div className="date-input-container">
          <label htmlFor="endingDate" className="date-label">
            Select ending date:
          </label>
          <input
            type="date"
            id="endingDate"
            name="endingDate"
            value={isEditing ? editFormValues.endingDate :formValues.endingDate}
            onChange={handleInputChange}
            className="date-input"
            required
          />
        </div>
      </div>
      <button type="button" className="add-task-button" onClick={addTask}>
        {isEditing? "Edit Task" :"Add Task"}
      </button>
    </form>
      <button type="button" className="display-task-button" onClick={handleSortTasks}>
        Sort by Date: older to newer
      </button>
      <button type="button" className="display-task-button" onClick={handleOpenTasks}>
        Open tasks
      </button>
      <button type="button" className="display-task-button" onClick={handleGetThisWeekTasks}>
        This week tasks
      </button>
      <button type="button" className="display-task-button" onClick={handleGetClosedTasks}>
        Closed tasks
      </button>
      <button type="button" className="display-task-button" onClick={handleGetAllTasks}>
        All tasks
      </button>
    </div>

    <button type="button" className="stats-task-button" onClick={() => setShowStatistics(!showStatistics)}>
      Statistics
    </button>
    
    {showStatistics && (
      <div className="statistics-container">
        <h2 className="statistics-heading">Statistics : select a time period to see who has the most tasks!</h2>
        <div className="statistics-content">
        <ul className="user-list">
            {/* Iterate over users and display them */}
            {users.map((user, index) => (
              <li key={index} className="user-item">
                {user}
              </li>
            ))}
          </ul>
          <div className="date-input-container">
            <label htmlFor="startDate" className="date-label">
              Select start date:
            </label>
            <input
              type="date"
              id="startDate"
              name="startDate"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="date-input"
              required
            />
          </div>
          <div className="date-input-container">
            <label htmlFor="endDate" className="date-label">
              Select end date:
            </label>
            <input
              type="date"
              id="endDate"
              name="endDate"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className="date-input"
              required
            />
          </div>
          <button type="button" className="add-task-button" onClick={handleStatisticsByPeriod}>
            submit
          </button>
        </div>
      </div>
    )}

    </div>
  );
  
};

export default Tasks;
