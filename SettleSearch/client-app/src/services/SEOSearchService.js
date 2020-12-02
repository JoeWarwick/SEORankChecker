const getResults = (type, term, site) => {
    return fetch(`/SEOSearch?type=${type}&term=${term}&uri=${site}`)
};

// eslint-disable-next-line import/no-anonymous-default-export
export default { getResults }