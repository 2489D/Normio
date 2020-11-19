import {config} from "../config/development";
import axios from 'axios';

export default class NormioApi {
    static backendUrl = config.backendUrl

    /* Queries */
    static async getExam(examId: string) {
        return await axios.get(this.backendUrl + '/exam', {
            params: { examId }
        })
    }

    /* Commands */
    static async openExam(title: string, startDateTime?: string, durationMins?: number) {
        return await axios.post(this.backendUrl + '/api/openExam', {
            title,
            startDateTime,
            durationMins,
        })
    }

    static async startExam(examId: string) {
        return await axios.post(this.backendUrl + '/api/startExam', {
            examId
        })
    }

    static async endExam(examId: string) {
        return await axios.post(this.backendUrl + '/api/endExam', {
            examId
        })
    }

    static async closeExam(examId: string) {
        return await axios.post(this.backendUrl + '/api/closeExam', {
            examId
        })
    }

    static async addStudent(examId: string, name: string) {
        return await axios.post(this.backendUrl + '/api/addStudent', {
            examId,
            name
        })
    }
    
    static async removeStudent(examId: string, studentId: string) {
        return await axios.post(this.backendUrl + '/api/removeStudent', {
            examId,
            studentId,
        })
    }

    static async addHost(examId: string, name: string) {
        return await axios.post(this.backendUrl + '/api/addHost', {
            examId,
            name
        })
    }

    static async removeHost(examId: string, hostId: string) {
        return await axios.post(this.backendUrl + '/api/removeHost', {
            examId,
            hostId,
        })
    }

    static async createSubmission(examId: string, studentId: string, title: string, description?: string) {
        return await axios.post(this.backendUrl + '/api/createSubmission', {
            examId,
            studentId,
            title,
            description,
        })
    }
    
    static async createQuestion(examId: string, hostId: string, title: string, description?: string) {
        return await axios.post(this.backendUrl + '/api/createSubmission', {
            examId,
            hostId,
            title,
            description,
        })
    }

    static async deleteQuestion(examId: string, questionId: string) {
        return await axios.post(this.backendUrl + '/api/createSubmission', {
            examId,
            questionId,
        })
    }

    static async sendMessage(examId: string, messageKind: number, senderId: string, receiverId: string[], content: string) {
        return await axios.post(this.backendUrl + '/api/createSubmission', {
            examId,
            messageKind,
            senderId,
            receiverId,
            content,
        })
    }

    static async changeExamTitle(examId: string, title: string) {
        return await axios.post(this.backendUrl + '/api/changeTitle', {
            examId,
            title,
        })
    }
    
}