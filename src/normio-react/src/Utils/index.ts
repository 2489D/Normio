export function formatTime(seconds: number) {
    const hours = Math.trunc(seconds / 3600)
    const minutes = Math.trunc((seconds - hours * 3600) / 60)
    const secs = Math.trunc(seconds - hours * 3600 - minutes * 60)
    return `${hours}시간 ${minutes}분 ${secs}초`
}