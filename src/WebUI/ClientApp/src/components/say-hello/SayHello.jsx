import React, { useEffect, useState } from 'react';
import authService from './../api-authorization/AuthorizeService'


const SayHello = () => {
    const [data, setData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {

                // Make a fetch request
                const token = await authService.getAccessToken();
                const response = await fetch('/api/TodoLists', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                const responseData = await response.json();
                setData(responseData);

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
                    <pre>{JSON.stringify(data, null, 2)}</pre>
                ) : (
                    <p>Loading...</p>
                )}
            </div>
        </div>
    )
}

export default SayHello;