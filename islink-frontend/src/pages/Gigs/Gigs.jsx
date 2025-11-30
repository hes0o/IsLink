import { useParams } from 'react-router-dom'
import './Gigs.css'

function Gigs() {
  const { category } = useParams()

  return (
    <div className="gigs-page">
      <div className="container">
        <h1>Browse Gigs {category && `- ${category.replace('-', ' ')}`}</h1>
        <p>Gigs listing coming soon...</p>
      </div>
    </div>
  )
}

export default Gigs

