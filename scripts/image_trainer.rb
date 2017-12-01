require 'net/http'
require 'uri'
require 'json'

def upload_file(tag, url)
	body_json = "{
  \"TagIds\": [
    \"#{tag}\"
  ],
  \"Urls\": [
    \"#{url}\"
  ]
}"

	uri = URI('https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/61f0cc4c-fb4f-4d62-95e1-2b2c631ae9b8/images/url')
	uri.query = URI.encode_www_form({
	})

	request = Net::HTTP::Post.new(uri.request_uri)
	# Request headers
	request['Content-Type'] = 'application/json'
	# Request headers
	request['Training-Key'] = '8939c29c60754789b9d42301602c1091'
	# Request body
	request.body = body_json

	response = Net::HTTP.start(uri.host, uri.port, :use_ssl => uri.scheme == 'https') do |http|
		http.request(request)
	end

	return response.body
end

accessKey = "3e36118e2fd547218018f5b759984b24"

uri  = "https://api.cognitive.microsoft.com"
path = "/bing/v7.0/images/search"
term = "Dodge Dart"

if accessKey.length != 32 then
    puts "Invalid Bing Search API subscription key!"
    puts "Please paste yours into the source code."
    abort
end
uri = URI(uri + path + "?q=" + URI.escape(term))
puts "Searching images for: " + term

request = Net::HTTP::Get.new(uri.request_uri)
request['Ocp-Apim-Subscription-Key'] = accessKey

response = Net::HTTP.start(uri.host, uri.port, :use_ssl => uri.scheme == 'https') do |http|
    http.request(request)
end

puts "\nRelevant Headers:\n\n"
response.each_header do |key, value|
    # header names are coerced to lowercase
    if key.start_with?("bingapis-") or key.start_with?("x-msedge-") then
        puts key + ": " + value
    end
end

puts "\nJSON Response:\n\n"
#puts JSON::pretty_generate(JSON(response.body))
vals = JSON.parse(response.body)
vals["value"].each do |f|
	new_url = f["contentUrl"]
	new_tag = "logos"
	upload_file(new_tag, new_url)
end

