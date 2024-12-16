import { FaArrowLeft } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import './HomeButton.css'

const HomeButton = () => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate('/home'); // Navigate to the home page
  };

  return (
    <div className="home-button" onClick={handleGoHome} role="button">
      <FaArrowLeft />
    </div>
  );
};

export default HomeButton;
