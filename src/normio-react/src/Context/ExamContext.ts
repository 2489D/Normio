import {createContext} from "react";

export declare namespace Normio {
    type Host = {
        id: string,
        name: string,
    }
    type Student = {
        id: string,
        name: string,
    }
    type Question = {
        id: string,
        examId: string,
        hostId: string,
        title: string,
        timestamp: string,
        description: string | null,
    }
    
    type ExamStatus =
        | { BeforeExam: {}}
        | { DuringExam: {}}
        | { AfterExam: {}}

    type Exam = {
        id: string,
        title: string,
        status: Normio.ExamStatus,
        startDateTime: string,
        durationMins: number,
        hosts: Normio.Host[],
        students: Normio.Student[],
        questions: Normio.Question[],
    }
}


type ExamContext = {
    exam: Normio.Exam | null,
    updateExam: (exam: Normio.Exam) => void,
}

export const ExamContext = createContext<ExamContext>({
    exam: null,
    updateExam: (exam: Normio.Exam) => {},
});