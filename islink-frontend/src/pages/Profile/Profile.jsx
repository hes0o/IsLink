import { useParams } from 'react-router-dom'
import './Profile.css'

function Profile() {
  const { username } = useParams()

  return (
    <div className="profile-page">
      <div className="container">
        <h1>Profile</h1>
        <p>Viewing profile: @{username}</p>
      </div>
    </div>
  )
}

export default Profile

