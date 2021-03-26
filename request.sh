while [ true ]
do
    curl -X GET localhost:1923/api/products
    echo get done
    curl -X POST http://localhost:1923/api/products -H "Content-Length: 0"
    echo post done
    curl -X PUT http://localhost:1923/api/products -H "Content-Length: 0"
    echo put done
    curl -X DELETE localhost:1923/api/products
    echo del done
done
