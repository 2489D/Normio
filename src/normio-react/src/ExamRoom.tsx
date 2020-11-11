import React from "react";

const ExamRoom: React.FC = () => {
    return (
        <div className={"container"}>
            <h1>Exam Room 123</h1>
            <div>
                <div>Exam Id : 123</div>
                <div>Hosts: Kang, Joo</div>
            </div>
            <h2>Video Chat</h2>
            <h2>Exam Questions</h2>
            <div>
                <button>Question 1</button>
            </div>
            <h2>Submit</h2>
            <div>
                <form>
                    <input type={"file"} />
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