import { API_BASE_URL } from './config';

const removeTask = (taskId, setTasks, setFormValues) => {

    fetch(`${API_BASE_URL}/remove-task`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({'uuid': taskId}),
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log('Task remove successfully:', data);

        setTasks(data)

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
      })
      .catch(error => {
        console.error('Error adding task:', error);
      });
  };

const checkInput = (formValues) => {
console.log('Checking input:', formValues);
if (!formValues.subject || !formValues.date || !formValues.endingDate || !formValues.urgency || !formValues.userInCharge) {
    alert('Please fill in all required fields (Subject, Date, Ending Date, Urgency, User in charge).');
    return false;
}

if (isNaN(formValues.urgency) || formValues.urgency < 1 || formValues.urgency > 5) {
    alert('Urgency must be a number between 1 and 5.');
    return false;
}
return true;
} 

const checkUser = async (username) => {
    const url = `${API_BASE_URL}/user-exist?user=` + encodeURIComponent(username) ;
    console.log(url)

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const result = await response.json();
            console.log("result  "+result)
            console.log("result  "+typeof(result))
            return true;
            
        } else {
            console.error('Login failed');
            return false;
        }
    } catch (error) {
        console.error('Error during login:', error);
    }
  };

  const sortTasks = async (setTasks) => {
    const url = `${API_BASE_URL}/sort-old-to-new?user=` + encodeURIComponent("user") ;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const result = await response.json();
            setTasks(result)
        } else {
            console.error('Login failed');
        }
    } catch (error) {
        console.error('Error during login:', error);
    }
  }; 
  

  const getOpenTasks = async (setTasks) => {
    const url = `${API_BASE_URL}/open-tasks?user=` + encodeURIComponent("user") ;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const result = await response.json();
            console.log(result)
            setTasks(result)
        } else {
            console.error('Login failed');
        }
    } catch (error) {
        console.error('Error during login:', error);
    }
    }; 

    const getThisWeekTasks = async (setTasks) => {
        const url = `${API_BASE_URL}/this-week-tasks?user=` + encodeURIComponent("user") ;
    
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
    
            if (response.ok) {
                const result = await response.json();
                setTasks(result)
            } else {
                console.error('Login failed');
            }
        } catch (error) {
            console.error('Error during login:', error);
        }
      };
      

const getAllTasks = async (setTasks) => {
const url = `${API_BASE_URL}/get-tasks?user=` + encodeURIComponent("user");

    try {
    const response = await fetch(url, {
        method: 'GET',
        headers: {
        'Content-Type': 'application/json',
        },
    });

    if (response.ok) {
        const result = await response.json();
        if (result.length > 0) {
        console.log('Tasks fetched successfully:', result);
        setTasks(result);
        }
        
    } else {
        console.error('Fetch failed');
    }
    } catch (error) {
    console.error('Error during fetch:', error);
    }
};  

const getClosedTasks = async (setTasks) => {
    const url = `${API_BASE_URL}/closed-tasks?user=` + encodeURIComponent("user") ;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const result = await response.json();
            setTasks(result)
        } else {
            console.error('Login failed');
        }
    } catch (error) {
        console.error('Error during login:', error);
    }
  };


const getStatisticsByPeriod = async (setUsers, startDate, endDate) => {
const url = `${API_BASE_URL}/statistics-get-best-users-by-period?start-date=` + encodeURIComponent(startDate) + '&end-date=' + encodeURIComponent(endDate);

    try {
    const response = await fetch(url, {
        method: 'GET',
        headers: {
        'Content-Type': 'application/json',
        },
    });

    if (response.ok) {
        const result = await response.json();
        console.log(result)
        setUsers(result);
    } else {
        console.error('Fetch failed');
    }
    } catch (error) {
    console.error('Error during fetch:', error);
    }
};  

const deleteUser = async (username) => {
    fetch(`${API_BASE_URL}/delete-user`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({'username': username}),
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log('User Deleted successfully:', data);
        return true;
      })
      .catch(error => {
        console.error('Error adding task:', error);
      });
};


  export { removeTask, checkInput, checkUser, sortTasks, getOpenTasks, getThisWeekTasks, getAllTasks, getClosedTasks, getStatisticsByPeriod, deleteUser };
