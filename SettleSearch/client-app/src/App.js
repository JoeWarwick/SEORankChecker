import React, { useState } from "react";
import SEOResultList from "./components/SEOResultList";
import './styles.css'

function App() {
  const initialState = { term: "e-settlements", site: "sympli.com.au"}
  const [item, setItem] = useState(initialState)

  const handleInputChange = event => {
		const { name, value } = event.target
		setItem({ ...item, [name]: value })
	}


  return (
    <div className="App">
      <header>
        <h1>SEO Rank Checker</h1>
        <div class="form">
          <label for="term">Search Term:</label>
          <input type="text" value={item.term} name="term" onChange={handleInputChange} />
          <label for="term">Site:</label>
          <input type="text" value={item.site} name="site" onChange={handleInputChange} />
          <button class="btn btn-primary" onClick={handleInputChange}>Go</button>
        </div>
      </header>
      <div className="content">
        <div class="grid-container">
          <div class="google-title">Google Results</div>
          <div class="bing-title">Bing Results</div>
          <div class="google-results">
            <SEOResultList searchEngine="Google" searchTerm={item.term} searchSite={item.site} />
          </div>
          <div class="bing-results">
            <SEOResultList searchEngine="Bing" searchTerm={item.term} searchSite={item.site} />
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
