import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import FindExam from "../Components/FindExam/FindExam";

const Exams: React.FC = props => {
    return (
        <div className="my-5">
            <FindExam />
        </div>
    )
}

export default Exams