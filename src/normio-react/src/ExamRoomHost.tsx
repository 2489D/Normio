import React, { useEffect } from "react";
import axios from 'axios';

const ExamRoom: React.FC = () => {
    const peers = [
        "peer 1",
        "peer 2",
    ]
    const questions = [
        "Question 1",
        "Question 2",
    ]

    const baseurl = 'localhost/6546';
    const url = baseurl + '/api/openExam';
    useEffect(() => {
        async function fetchData(){
            const result = await axios.get(url, );
            console.log(result.data);
        }
        fetchData();
    }, []);

    return (
        <div className={"container"}>
            <h1>Exam Room</h1>
            <div>
                <div>Exam Id : 123</div>
                <div>Hosts: Kang, Joo</div>
            </div>
            <h2>Video Chat</h2>
            <div>
                {peers.map((peer, index) => (
                    <div key={`${peer}-${index}`} className={"m-1"}>
                        {peer}
                    </div>
                ))}
            </div>
            <h2>Exam Questions</h2>
            <div>
                {questions.map((q, index) => (
                    <div key={`${q}-${index}`} className={"p-1"}>
                        <button>{q}</button>
                    </div>
                ))}
            </div>
            <h2>Submit</h2>
            <div>
                <form onSubmit={e => alert(JSON.stringify(e))}>
                    <input type={"file"} />
                    <input type={"submit"} />
                </form>
            </div>
            <h2>Ask</h2>
            <div>
                <form>
                    
                </form>
            </div>
        </div>
    );
}

export default ExamRoom;