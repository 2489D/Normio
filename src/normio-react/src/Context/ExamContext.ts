import {createContext} from "react";

export type ExamReadModel = {
    title: string
    status: any,
    startDateTime: string,
    durationMins: number,
}

type ExamContext = {
    exam: ExamReadModel | null,
    updateExam: (data: ExamReadModel) => void,
}

export const ExamContext = createContext<ExamContext>({
    exam: null,
    updateExam: (data: ExamReadModel) => {},
});