import React, { useEffect, useState } from 'react';
import authService from './../api-authorization/AuthorizeService'

function sendHttpRequest(url, accessToken) {
   

    // Create a new XMLHttpRequest object
    const xhr = new XMLHttpRequest();

    // Configure the request
    xhr.open('GET', url, true);
    xhr.setRequestHeader('Authorization', `Bearer ${accessToken}`);
    //xhr.setRequestHeader('Content-Type', 'application/json');

    // Set up event listeners
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            // Request was successful, handle the response
            const response = JSON.parse(xhr.responseText);
            return response;
        } else {
            // Request failed
            console.error('Request failed with status:', xhr.status);
        }
    };

    xhr.onerror = function () {
        // Network error
        console.error('Network error occurred');
    };

    // Send the request
    xhr.send();
}



const SayHello = () => {
    const [data, setData] = useState(null);

    // Effect to fetch data when the component mounts
    useEffect(() => {
        const fetchData = async () => {
            try {
                // Make a fetch request
                const token = await authService.getAccessToken();
                var dat = sendHttpRequest("https://localhost:5503/api/TodoLists", token);


            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        // Call the fetch function
        fetchData();
    }, []);


    return (
        <div>
            <h1>Hello!</h1>
            <p>Welcome to the SayHello component in React.</p>

            <div>
                <h1>Data fetching</h1>
                {data ? (
                    // Display the fetched data if available
                    <pre>{JSON.stringify(data, null, 2)}</pre>
                ) : (
                    // Display a loading message while fetching data
                    <p>Loading...</p>
                )}
            </div>
        </div>
    )
}

export default SayHello;