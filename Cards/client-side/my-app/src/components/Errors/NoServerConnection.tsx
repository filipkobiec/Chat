import { Button } from 'react-bootstrap';
import styles from './NoServerConnection.module.scss'; 


function NoServerConnection(){
    return (
        <div className={styles.errorContainer}>
            <div className={styles.errorMessage}>
                <h2>There was a problem with server connection please try again</h2>
                <Button href='/'>Reload</Button>
            </div>
        </div>
    )
}

export default NoServerConnection;