import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import FindExamCard from "../Components/FindExam/FindExam";

const FindExam: React.FC = props => {
    return (
        <div className="my-5">
            <FindExamCard />
        </div>
    )
}

export default FindExam