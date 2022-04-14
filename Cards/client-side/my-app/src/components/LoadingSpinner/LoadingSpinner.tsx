import styles from './LoadingSpinner.module.scss'; 


function LoadingSpinner(){
    return (
        <div className={styles.spinnerContainer}>
            <div className={styles.loadingSpinner}></div>
        </div>
    )
}

export default LoadingSpinner;