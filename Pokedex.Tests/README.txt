AsyncEnumerator allows us to better Moq .ToListAsync. 

By implementing a few classes with specific interfaces, Moqing works as expected for DbSets.

See here for reference: 
	https://entityframeworkcore.com/knowledge-base/47965574/unit-testing-with-moq-sometimes-fails-on-tolistasync--
	(Note: my implementation is slightly different but nearly identical)