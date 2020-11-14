import React, { useState, useEffect } from 'react'

type CurrentTimeProps = {
    variant?: string
}

const CurrentTime: React.FC<CurrentTimeProps> = props => {
    const [currentTime, setCurrentTime] = useState(new Date().toLocaleTimeString());

    useEffect(() => {
        setInterval(() => {
            setCurrentTime(new Date().toLocaleTimeString())
        }, 1000)
    }, [])

    return (
        <div className={props.variant}>
            {currentTime}
        </div>
    )
}

export default CurrentTime