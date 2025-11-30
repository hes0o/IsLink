import { useParams } from 'react-router-dom'
import './GigDetails.css'

function GigDetails() {
  const { id } = useParams()

  return (
    <div className="gig-details-page">
      <div className="container">
        <h1>Gig Details</h1>
        <p>Viewing gig: {id}</p>
      </div>
    </div>
  )
}

export default GigDetails

