import React, { useState, useEffect } from "react"
import SkeletonDefinition from "./skeletons/SkeletonDefinition"
import SEOSearchService from '../services/SEOSearchService'

const SEOResultList = props => {
    const [resultList, setResultList] = useState([])
    const [isLoading, setIsLoading] = useState(true)
    const [isError, setIsError] = useState(false)
    const [error, setError] = useState('')
    
    useEffect(() => {
        setIsLoading(true)        
        SEOSearchService.getResults(props.searchEngine, props.searchTerm, props.searchSite)
        .then(async(res) => {
            // status 404 or 500 set as errors
            if(res.ok){
                var json = await res.json()
                return json
            }
            else throw new Error(res.statusText)
        })
        .then(res => setResultList(res))
        .then(() => { 
          setIsLoading(false)
          setError(false)
        })
        .catch(error => {
            setIsError(true)
            setError(`${props.searchEngine} Service : ${error.message}`)
            setIsLoading(false)
            console.log(`${props.searchEngine} Service : ${error.message}`)
        })        
      }, [props.searchEngine, props.searchTerm, props.searchSite])

      return (
          <div>
            <div className="seoresult-list header">
                <div className="h1">Position</div>
                <div className="h2">Entry</div>
            </div>
            {isLoading && [1,2,3].map(n => <SkeletonDefinition key={n} theme="light" />)}
            
            {!isLoading && resultList.sort((a,b) => a.Position < b.Position).map((c, index) => (
                <div className="seoresult-list" key={index}>
                    <div className="c1">
                    <h4 style={{ textDecoration: "none" }}>
                        {c.position}
                    </h4>
                    </div>
                    <div className="c2">
                    <p>{c.title}</p>
                    </div>
                </div>
            ))}

            {isError && (
                <div class="alert alert-danger" role="alert">
                    { error }
                </div>
            )}
          </div>
      )
}

export default SEOResultList