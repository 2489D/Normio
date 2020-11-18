import {config} from "../config/development";
import axios from 'axios';

export default class NormioApi {
    static backendUrl = config.backendUrl

    static async getExam(examId: string) {
        return await axios.get(this.backendUrl + '/exam', {
            params: { examId }
        })
    }
    
    static async openExam(title: string, startDateTime?: string, durationMins?: number) {
        return await axios.post(this.backendUrl + '/api/openExam', {
            title,
            startDateTime,
            durationMins,
        })
    }
    
}